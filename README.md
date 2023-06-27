# Observability Workŝhop

This is a sample project used to demonstrate how Observability can be implemented by using [OpenTelemetry](https://opentelemetry.io/). A [C2 diagram](https://github.com/GasyTek/observability/tree/main/docs) is available that describes the different components used and their interactions.

## Components :

- OpenTelemetry Collector : to collect metrics from all components
- Prometheus / Grafana : to visualize metrics
- Jaeger : to visualize distributed traces
- Elasticsearch / Elastic APM / Kibana : to visualize logs and distributed traces
- Sentry : to monitor unexpected errors
- Redis 
- MongoDB
- Postgres

## Quick start :
### Prerequisites :
Docker & Docker compose
### Getting started :

 1. Run docker compose file
```bash
    cd ./deployment
    docker compose up -d
```
2. Once every containers are up and running, configure Elastic APM on Kibana by following instructions described [here](https://www.elastic.co/guide/en/apm/guide/current/apm-server-configuration.html)
3. Open **Observability Workshop.sln** using your favorite IDE (e.g Visual Studio or Rider)
3. Run both contained project in parallel.

## Notes :

### Api Gateway 

- http://localhost:5000/products
- http://localhost:5000/products/{productId}

### Jaeger

- http://localhost:16686/search

### Kibana

- Endpôint: http://localhost:5601
- Credentials: elastic / elastic

### Mongo Express
- http://localhost:8081/

### Prometheus
- http://localhost:9090/

### Grafana
- Endpoint: http://localhost:3000/
- Credentials: admin / admin
- Use http://prometheus:9090 as datasource URL for Prometheus