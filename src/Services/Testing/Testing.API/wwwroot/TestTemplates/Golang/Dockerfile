FROM golang:1.17-alpine AS builder

WORKDIR /app

COPY main.go .

RUN go build -o app main.go

FROM alpine:3.14

COPY --from=builder /app/app .

CMD ["./app"]