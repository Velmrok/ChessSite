FROM python:3.11-slim

WORKDIR /app

RUN pip install --no-cache-dir torch --index-url https://download.pytorch.org/whl/cpu \
 && pip install --no-cache-dir flask python-chess orjson zstandard

COPY . /app

ENV MODEL_PATH=/app/models/best_model.pth
EXPOSE 5000
CMD ["python", "server.py"]
