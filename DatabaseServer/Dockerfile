FROM python:3.9-slim

EXPOSE 5001

COPY requirements.txt requirements.txt
RUN pip3 install --no-cache-dir -r requirements.txt
WORKDIR /workspace

COPY  . . 

CMD ["python3", "main.py"]
