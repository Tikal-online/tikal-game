ARG NODE_VERSION=24.16.0-alpine
ARG NGINX_VERSION=stable-alpine3.23-slim

# =========================================
# Stage 1: Builder
# =========================================
FROM node:${NODE_VERSION} AS builder

WORKDIR /app

COPY TikalFrontend/package*.json ./

RUN npm ci --ignore-scripts

COPY TikalFrontend/ .

ARG ENV=production

RUN npm run build --configuration="${ENV}"

# =========================================
# Stage 2: Prepare Nginx
# =========================================
FROM nginxinc/nginx-unprivileged:${NGINX_VERSION} AS runner

COPY TikalFrontend/.nginx/nginx.conf /etc/nginx/conf.d/default.conf

COPY --chown=root:nginx --chmod=750 --from=builder app/dist/TikalFrontend/browser /var/www

USER nginx

EXPOSE 80

ENTRYPOINT ["nginx", "-c", "/etc/nginx/nginx.conf"]
CMD ["-g", "daemon off;"]