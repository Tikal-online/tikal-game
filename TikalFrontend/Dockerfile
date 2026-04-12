FROM node:24-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm ci

RUN npm install -g @angular/cli

COPY . .

ARG ENV=production

RUN ng build --configuration=${ENV}

FROM nginx:1.29.0-alpine

COPY .nginx/nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build app/dist/TikalFrontend/browser /var/www

EXPOSE 80