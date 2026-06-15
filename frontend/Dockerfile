FROM node:20-alpine AS deps
WORKDIR /app
COPY package*.json ./
RUN npm ci

FROM node:20-alpine AS build
WORKDIR /app

COPY --from=deps /app/node_modules ./node_modules
COPY . .

ARG VITE_KEYCLOAK_AUTHORITY
ARG VITE_KEYCLOAK_REDIRECT_URI
ARG VITE_KEYCLOAK_POST_LOGOUT_URI


ENV VITE_KEYCLOAK_AUTHORITY=$VITE_KEYCLOAK_AUTHORITY
ENV VITE_KEYCLOAK_REDIRECT_URI=$VITE_KEYCLOAK_REDIRECT_URI
ENV VITE_KEYCLOAK_POST_LOGOUT_URI=$VITE_KEYCLOAK_POST_LOGOUT_URI

RUN npm run build


FROM nginx:alpine AS production

RUN chown -R nginx:nginx /var/cache/nginx /var/log/nginx /etc/nginx/conf.d /run /usr/share/nginx/html

ARG NODE_ENV=production
ARG BUILD_DATE
ARG VERSION
ARG NGINX_CONF=nginx/default.conf

LABEL org.opencontainers.image.created=${BUILD_DATE} \
      org.opencontainers.image.version=${VERSION} \
      org.opencontainers.image.title=${NODE_ENV} 
      
USER nginx

COPY --from=build --chown=nginx:nginx /app/dist /usr/share/nginx/html

COPY --chown=nginx:nginx ${NGINX_CONF} /etc/nginx/conf.d/default.conf

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD wget -q http://localhost:80/ || exit 1

CMD ["nginx", "-g", "daemon off;"]
