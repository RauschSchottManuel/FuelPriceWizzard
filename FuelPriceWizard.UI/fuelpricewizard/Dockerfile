# Stage 1: build the Angular application
FROM node:20-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

# Stage 2: serve the static build with nginx
FROM nginx:1.30-alpine AS final

# OCI image metadata — surfaced by the zot registry UI (vendor, title, source, …).
# Predefined org.opencontainers.image.* keys; see https://github.com/opencontainers/image-spec/blob/main/annotations.md
LABEL org.opencontainers.image.title="FuelPriceWizard UI" \
      org.opencontainers.image.description="Angular single-page app for FuelPriceWizard, served as static files via nginx with an /api reverse proxy to the FuelPriceWizard API." \
      org.opencontainers.image.vendor="Manuel Rausch-Schott" \
      org.opencontainers.image.authors="Manuel Rausch-Schott <manuel.rausch.schott@gmail.com>" \
      org.opencontainers.image.url="https://github.com/RauschSchottManuel/FuelPriceWizzard" \
      org.opencontainers.image.source="https://github.com/RauschSchottManuel/FuelPriceWizzard" \
      org.opencontainers.image.documentation="https://github.com/RauschSchottManuel/FuelPriceWizzard" \
      org.opencontainers.image.licenses="UNLICENSED" \
      org.opencontainers.image.base.name="docker.io/library/nginx:1.30-alpine"

# API upstream the /api reverse proxy forwards to; override at runtime, e.g.
#   docker run -e API_URL=http://fuelpricewizard-api:8080 ...
ENV API_URL=http://fuelpricewizard-api:8080
# Makes the entrypoint export NGINX_LOCAL_RESOLVERS (the container's DNS servers),
# which the config template uses for request-time resolution of API_URL.
ENV NGINX_ENTRYPOINT_LOCAL_RESOLVERS=1
COPY nginx/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist/fuelpricewizard/browser /usr/share/nginx/html
EXPOSE 80
