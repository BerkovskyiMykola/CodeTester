FROM node:19-alpine3.16

ADD app.js .

ENTRYPOINT [ "timeout", "{timeLimit}", "node", "./app.js" ]