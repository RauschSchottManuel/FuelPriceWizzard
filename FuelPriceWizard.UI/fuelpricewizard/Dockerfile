# Stage 1: build the Angular application
FROM node:20-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

# Stage 2: serve the static build with nginx
FROM nginx:1.27-alpine AS final
# API upstream the /api reverse proxy forwards to; override at runtime, e.g.
#   docker run -e API_URL=http://fuelpricewizard-api:8080 ...
ENV API_URL=http://fuelpricewizard-api:8080
# Makes the entrypoint export NGINX_LOCAL_RESOLVERS (the container's DNS servers),
# which the config template uses for request-time resolution of API_URL.
ENV NGINX_ENTRYPOINT_LOCAL_RESOLVERS=1
COPY nginx/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist/fuelpricewizard/browser /usr/share/nginx/html
EXPOSE 80
