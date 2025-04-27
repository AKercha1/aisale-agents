# The code is designed to find mid-sized insurance companies in the USA using Google search and store the results in a PostgreSQL database.

import json  # Importing the json module to parse JSON strings

area = Parameters["parameter1"]

# Step 1: Generate query strings for searching
queries_json = ExecuteAgent("company-search-generate-prompt", ["3", "USA", area, "50-500", "Google"])

# Step 2: Parse the JSON string to extract the queries
queries = json.loads(queries_json)["queries"]  # Parsing the JSON to get the list of queries

# Step 3: Perform Google search for each query and collect results
search_results = []
for query in queries:
    result = ExecuteAgent("google-search-tool", [query, "10", "0"])  # Searching with a limit of 10 results
    search_results.append(result)

# Step 4: Prepare to insert results into the database
for i, query in enumerate(queries):
    # Count the number of results obtained
    result_count = len(search_results[i])
    
    # Construct SQL query to insert into the database
    sql_query = f"""
    INSERT INTO public.search_query_log (search_engine, query_text, search_count, result_count, result)
    VALUES ('Google', '{query}', {result_count}, {result_count}, '{json.dumps(search_results[i])}');
    """
    
    # Execute the SQL query
    ExecuteAgent("sql-execute", [sql_query])

# Final result is an empty string as per the requirement
result = ""