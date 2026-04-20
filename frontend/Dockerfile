FROM node:20-alpine AS deps
WORKDIR /app
COPY package*.json ./
RUN npm ci

FROM node:20-alpine AS build
WORKDIR /app

COPY --from=deps /app/node_modules ./node_modules
COPY . .
RUN npm run build


FROM nginx:alpine AS production

RUN chown -R nginx:nginx /var/cache/nginx /var/log/nginx /etc/nginx/conf.d /run /usr/share/nginx/html

ARG NODE_ENV=production
ARG BUILD_DATE
ARG VERSION

LABEL org.opencontainers.image.created=${BUILD_DATE} \
      org.opencontainers.image.version=${VERSION} \
      org.opencontainers.image.title=${NODE_ENV} 
      
USER nginx

COPY --from=build --chown=nginx:nginx /app/dist /usr/share/nginx/html

COPY --chown=nginx:nginx nginx/default.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD wget -q http://localhost:3000/ || exit 1

CMD ["nginx", "-g", "daemon off;"]
