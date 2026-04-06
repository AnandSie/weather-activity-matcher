# Stage 1: Build React app
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
# VITE_API_BASE_URL is empty so Vite uses relative /api paths (nginx proxies them)
RUN npm run build

# Stage 2: Serve with nginx
FROM nginx:alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
