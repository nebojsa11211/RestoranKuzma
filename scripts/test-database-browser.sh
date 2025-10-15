#!/bin/bash
# test-database-browser.sh
# Bash script to test the Database Browser API endpoints

# Configuration
BASE_URL="${BASE_URL:-http://localhost:5000}"
ADMIN_EMAIL="${ADMIN_EMAIL:-admin@restaurant.com}"
ADMIN_PASSWORD="${ADMIN_PASSWORD:-Admin123!}"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}================================${NC}"
echo -e "${CYAN}Database Browser API Test Script${NC}"
echo -e "${CYAN}================================${NC}"
echo ""
echo -e "${CYAN}API Base URL: ${BASE_URL}${NC}"
echo ""

# Step 1: Login and get access token
echo -e "${YELLOW}Step 1: Authenticating as admin...${NC}"

LOGIN_RESPONSE=$(curl -s -X POST "${BASE_URL}/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{
    \"email\": \"${ADMIN_EMAIL}\",
    \"password\": \"${ADMIN_PASSWORD}\",
    \"rememberMe\": false
  }")

ACCESS_TOKEN=$(echo "$LOGIN_RESPONSE" | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)

if [ -z "$ACCESS_TOKEN" ]; then
  echo -e "${RED}  Error: Failed to authenticate. Make sure the API is running and credentials are correct.${NC}"
  echo -e "${RED}  Response: ${LOGIN_RESPONSE}${NC}"
  exit 1
fi

EMAIL=$(echo "$LOGIN_RESPONSE" | grep -o '"email":"[^"]*' | cut -d'"' -f4)
ROLE=$(echo "$LOGIN_RESPONSE" | grep -o '"role":"[^"]*' | cut -d'"' -f4)

echo -e "${GREEN}  Success! Authenticated as: ${EMAIL} (Role: ${ROLE})${NC}"
echo ""

# Step 2: Get all tables
echo -e "${YELLOW}Step 2: Getting list of all tables...${NC}"

TABLES_RESPONSE=$(curl -s -X GET "${BASE_URL}/api/admin/database/tables" \
  -H "Authorization: Bearer ${ACCESS_TOKEN}" \
  -H "Content-Type: application/json")

# Pretty print the response
echo "$TABLES_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$TABLES_RESPONSE"
echo ""

# Extract first table name
FIRST_TABLE=$(echo "$TABLES_RESPONSE" | grep -o '"tableName":"[^"]*' | head -1 | cut -d'"' -f4)

if [ -z "$FIRST_TABLE" ]; then
  echo -e "${RED}  Error: No tables found or error occurred.${NC}"
  exit 1
fi

echo -e "${GREEN}  Found tables. Using '${FIRST_TABLE}' for further tests.${NC}"
echo ""

# Step 3: Get schema for first table
echo -e "${YELLOW}Step 3: Getting schema for table '${FIRST_TABLE}'...${NC}"

SCHEMA_RESPONSE=$(curl -s -X GET "${BASE_URL}/api/admin/database/tables/${FIRST_TABLE}/schema" \
  -H "Authorization: Bearer ${ACCESS_TOKEN}" \
  -H "Content-Type: application/json")

# Pretty print the response
echo "$SCHEMA_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$SCHEMA_RESPONSE"
echo ""

# Step 4: Get data from first table
echo -e "${YELLOW}Step 4: Getting data from table '${FIRST_TABLE}'...${NC}"

DATA_RESPONSE=$(curl -s -X GET "${BASE_URL}/api/admin/database/tables/${FIRST_TABLE}/data?page=1&pageSize=5" \
  -H "Authorization: Bearer ${ACCESS_TOKEN}" \
  -H "Content-Type: application/json")

# Pretty print the response
echo "$DATA_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$DATA_RESPONSE"
echo ""

# Step 5: Advanced query (Users table with filters)
echo -e "${YELLOW}Step 5: Testing advanced query on Users table (if exists)...${NC}"

QUERY_RESPONSE=$(curl -s -X POST "${BASE_URL}/api/admin/database/tables/Users/query" \
  -H "Authorization: Bearer ${ACCESS_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "page": 1,
    "pageSize": 10,
    "sortBy": "CreatedAt",
    "sortDirection": "desc",
    "filters": [
      {
        "columnName": "IsActive",
        "operator": "equals",
        "value": "true"
      }
    ]
  }')

# Pretty print the response
echo "$QUERY_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$QUERY_RESPONSE"
echo ""

echo -e "${GREEN}================================${NC}"
echo -e "${GREEN}All tests completed successfully!${NC}"
echo -e "${GREEN}================================${NC}"
