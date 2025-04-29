# Troubleshooting Guide

## Table of Contents
1. [Service Issues](#service-issues)
2. [Database Issues](#database-issues)
3. [Performance Issues](#performance-issues)
4. [Security Issues](#security-issues)
5. [Network Issues](#network-issues)
6. [Kubernetes Issues](#kubernetes-issues)
7. [Monitoring and Logging Issues](#monitoring-and-logging-issues)
8. [CI/CD Pipeline Issues](#cicd-pipeline-issues)

## Service Issues

### Service Not Starting

1. **Check Service Logs**
   ```bash
   # For Kubernetes
   kubectl logs -n ecommerce -l app=user-service
   
   # For Docker
   docker logs user-service
   ```

2. **Check Service Status**
   ```bash
   # For Kubernetes
   kubectl describe pod -n ecommerce -l app=user-service
   
   # For Docker
   docker inspect user-service
   ```

3. **Check Environment Variables**
   ```bash
   # For Kubernetes
   kubectl exec -it -n ecommerce user-service-pod -- env
   
   # For Docker
   docker exec user-service env
   ```

### Service Health Issues

1. **Check Health Endpoints**
   ```bash
   # Check health endpoint
   curl http://user-service/health
   
   # Check readiness endpoint
   curl http://user-service/ready
   
   # Check liveness endpoint
   curl http://user-service/live
   ```

2. **Check Dependencies**
   ```bash
   # Check database connectivity
   kubectl exec -it -n ecommerce user-service-pod -- nc -zv db-host 1433
   
   # Check Redis connectivity
   kubectl exec -it -n ecommerce user-service-pod -- nc -zv redis 6379
   ```

## Database Issues

### Connection Issues

1. **Check Database Connectivity**
   ```bash
   # Test database connection
   kubectl exec -it -n ecommerce mysql-pod -- mysql -h db-host -u user -p
   
   # Check connection string
   kubectl get secret -n ecommerce db-credentials -o jsonpath='{.data.connectionString}' | base64 --decode
   ```

2. **Check Database Status**
   ```bash
   # Check MySQL status
   kubectl exec -it -n ecommerce mysql-pod -- mysql -e "SHOW STATUS;"
   
   # Check PostgreSQL status
   kubectl exec -it -n ecommerce postgres-pod -- psql -c "SELECT * FROM pg_stat_activity;"
   ```

### Performance Issues

1. **Check Slow Queries**
   ```bash
   # MySQL slow queries
   kubectl exec -it -n ecommerce mysql-pod -- mysql -e "SHOW PROCESSLIST;"
   
   # PostgreSQL slow queries
   kubectl exec -it -n ecommerce postgres-pod -- psql -c "SELECT * FROM pg_stat_activity WHERE state = 'active';"
   ```

2. **Check Table Statistics**
   ```bash
   # MySQL table statistics
   kubectl exec -it -n ecommerce mysql-pod -- mysql -e "ANALYZE TABLE orders;"
   
   # PostgreSQL table statistics
   kubectl exec -it -n ecommerce postgres-pod -- psql -c "ANALYZE orders;"
   ```

## Performance Issues

### High Latency

1. **Check Service Latency**
   ```bash
   # Check API latency
   curl -o /dev/null -s -w "%{time_total}\n" http://api-gateway/metrics
   
   # Check database latency
   kubectl exec -it -n ecommerce mysql-pod -- mysql -e "SHOW PROFILE;"
   ```

2. **Check Resource Usage**
   ```bash
   # Check CPU usage
   kubectl top pods -n ecommerce
   
   # Check memory usage
   kubectl exec -it -n ecommerce user-service-pod -- dotnet-counters monitor
   ```

### Memory Issues

1. **Check Memory Usage**
   ```bash
   # Check pod memory
   kubectl top pods -n ecommerce
   
   # Check container memory
   docker stats user-service
   ```

2. **Check Garbage Collection**
   ```bash
   # Check .NET GC
   kubectl exec -it -n ecommerce user-service-pod -- dotnet-counters monitor
   
   # Check Java GC
   kubectl exec -it -n ecommerce java-service-pod -- jstat -gcutil 1
   ```

## Security Issues

### Authentication Issues

1. **Check Azure AD B2C**
   ```bash
   # Check token validation
   curl -v -H "Authorization: Bearer $TOKEN" http://api-gateway/validate
   
   # Check user session
   kubectl exec -it -n ecommerce user-service-pod -- curl http://localhost/auth/session
   ```

2. **Check Certificate Issues**
   ```bash
   # Check certificate validity
   openssl s_client -connect api-gateway:443 -servername api-gateway
   
   # Check certificate chain
   kubectl exec -it -n ecommerce api-gateway-pod -- openssl x509 -in /etc/ssl/certs/api-gateway.crt -text
   ```

## Network Issues

### Connectivity Issues

1. **Check Network Policies**
   ```bash
   # List network policies
   kubectl get networkpolicies -n ecommerce
   
   # Check policy details
   kubectl describe networkpolicy allow-api-gateway -n ecommerce
   ```

2. **Check Service Mesh**
   ```bash
   # Check Istio configuration
   istioctl analyze
   
   # Check service mesh traffic
   istioctl proxy-status
   ```

## Kubernetes Issues

### Cluster Issues

1. **Check Cluster Health**
   ```bash
   # Check cluster events
   kubectl get events --sort-by='.lastTimestamp'
   
   # Check node status
   kubectl describe nodes
   ```

2. **Check Resource Issues**
   ```bash
   # Check pod evictions
   kubectl get events --field-selector reason=Evicted
   
   # Check resource quotas
   kubectl describe resourcequota -n ecommerce
   ```

## Monitoring and Logging Issues

### Prometheus Issues

1. **Check Metrics Collection**
   ```bash
   # Check targets
   curl http://prometheus:9090/api/v1/targets
   
   # Check rules
   curl http://prometheus:9090/api/v1/rules
   ```

2. **Check Alert Manager**
   ```bash
   # Check alerts
   curl http://alertmanager:9093/api/v2/alerts
   
   # Check silences
   curl http://alertmanager:9093/api/v2/silences
   ```

### Logging Issues

1. **Check Elasticsearch**
   ```bash
   # Check cluster health
   curl http://elasticsearch:9200/_cluster/health
   
   # Check indices
   curl http://elasticsearch:9200/_cat/indices
   ```

2. **Check Fluentd**
   ```bash
   # Check Fluentd status
   kubectl logs -n logging -l app=fluentd
   
   # Check buffer status
   curl http://fluentd:24220/api/plugins.json
   ```

## CI/CD Pipeline Issues

### Build Issues

1. **Check Build Logs**
   ```bash
   # Check Azure DevOps build
   az pipelines runs show --id $BUILD_ID
   
   # Check GitHub Actions build
   gh run view $RUN_ID
   ```

2. **Check Test Results**
   ```bash
   # Check test results
   dotnet test --logger "console;verbosity=detailed"
   
   # Check coverage
   dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
   ```

### Deployment Issues

1. **Check Deployment Status**
   ```bash
   # Check Kubernetes deployment
   kubectl rollout status deployment/user-service -n ecommerce
   
   # Check deployment history
   kubectl rollout history deployment/user-service -n ecommerce
   ```

2. **Check Configuration**
   ```bash
   # Check ConfigMap
   kubectl describe configmap user-service-config -n ecommerce
   
   # Check Secrets
   kubectl describe secret user-service-secrets -n ecommerce
   ``` 