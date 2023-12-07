docker compose down
git pull
docker compose build myapp --no-cache
docker compose up -d
docker compose logs -f myapp
