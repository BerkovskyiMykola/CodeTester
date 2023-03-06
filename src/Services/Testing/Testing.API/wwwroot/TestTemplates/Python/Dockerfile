FROM python:alpine3.17

ADD main.py .

ENTRYPOINT [ "timeout", "{timeLimit}", "python", "./main.py"]