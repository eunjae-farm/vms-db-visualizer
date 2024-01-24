FROM python:3.9-slim

ENV USERNAME A
ENV PASSWORD B
ENV SSH_IP C
ENV SSH_PORT D

RUN apt update && \
        apt install -y sshuttle sshpass && \
        rm -rf /var/lib/apt/lists/*

EXPOSE 5001

COPY requirements.txt requirements.txt
RUN pip3 install --no-cache-dir -r requirements.txt
WORKDIR /workspace

COPY  . . 

CMD ["/workspace/entrypoint.sh" ]
