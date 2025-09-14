# Stage 1: build
FROM node:22.15.1 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Stage 2: production with nginx
FROM nginx:alpine

# Skopiuj własny config nginx, żeby obsłużyć SPA
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Skopiuj zbudowane pliki frontendu
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

