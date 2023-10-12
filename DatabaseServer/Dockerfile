FROM python:3.11-slim

EXPOSE 5001

RUN pip3 install --no-cache-dir -r requirements.txt
WORKDIR /workspace

COPY  . . 

CMD ["python3", "main.py"]
