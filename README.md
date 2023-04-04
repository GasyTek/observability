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

## Notes :
To access Kibana, use following credentials : elastic / elastic