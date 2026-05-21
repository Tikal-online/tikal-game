# =========================================
# Stage 1: Builder
# =========================================
FROM dhi.io/node:24-alpine3.22-dev AS builder

WORKDIR /app

COPY TikalFrontend/package*.json ./

RUN npm ci --ignore-scripts

COPY TikalFrontend/ .

ARG ENV=production

RUN npm run build --configuration="${ENV}"

# =========================================
# Stage 2: Prepare Nginx
# =========================================
FROM dhi.io/nginx:1.28.0-alpine3.21-dev AS runner

COPY TikalFrontend/.nginx/nginx.conf /etc/nginx/conf.d/default.conf

COPY --chown=nginx:nginx --from=builder app/dist/TikalFrontend/browser /var/www

USER nginx

EXPOSE 80

ENTRYPOINT ["nginx", "-c", "/etc/nginx/nginx.conf"]
CMD ["-g", "daemon off;"]