#!/bin/bash

# Wait for SQL Server to start up
echo "Waiting for SQL Server to start up..."
sleep 30s

# Run the SQL scripts
for file in /sqlscripts/*.sql; do
    echo "Running script: $file"
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Your_password123 -d master -i $file
done

echo "SQL scripts executed."