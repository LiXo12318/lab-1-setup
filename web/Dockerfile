# Stage 1: Build
FROM node:24-alpine AS builder
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

# Stage 2: Runner
FROM node:24-alpine AS runner
WORKDIR /app
ENV NODE_ENV production

# Створюємо користувача для безпеки
RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 vitejs

COPY --from=builder /app/dist ./dist
RUN npm install -g serve

USER vitejs
EXPOSE 3000
CMD ["serve", "-s", "dist", "-l", "3000"]