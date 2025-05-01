import json
from pathlib import Path
import psycopg2

# Get the directory where the .py file is located
script_dir = Path(__file__).parent

print("############################ Load films into DB ")

# Load the JSON file
with open(f'{script_dir}\\jsn-newmovies.json', 'r') as file:
    data = json.load(file)

print("############################ JSON File: ", file.name, "\n")

print("----- DB INFOS -----")
# Connect to the database
conn = psycopg2.connect(
    dbname='moviesdb',
    user='postgres',
    password='superdoopersecret',
    host='localhost',
    port='5432'
)
# ---
params = conn.get_dsn_parameters()
print(f"{'dbname:':<10} {params['dbname']}")
print(f"{'user:':<10} {params['user']}")
print(f"{'host:':<10} {params['host']}")
print(f"{'port:':<10} {params['port']}")
print("----- DB INFOS -----")

cursor = conn.cursor()

# Prepare insert statements
movie_insert_query = """
    INSERT INTO movies (id, title, slug, yearofrelease)
    VALUES (%s, %s, %s, %s)
    ON CONFLICT (slug) DO NOTHING
"""

genre_insert_query = """
    INSERT INTO genres (movieid, name)
    VALUES (%s, %s)
    -- ON CONFLICT (movieid, name) DO NOTHING
"""

# Loop through each movie object in the data
for movie in data:
    # Insert into movies table
    cursor.execute(
        movie_insert_query,
        (
            movie['Id'],
            movie['Title'],
            movie['Slug'],
            movie['YearOfRelease']
        )
    )

    # Insert related genres into genres table
    genre_tuples = [(movie['Id'], genre) for genre in movie['Genres']]
    cursor.executemany(genre_insert_query, genre_tuples)

# Commit and close
conn.commit()
# conn.rollback()  # Rollback to avoid any issues with the transaction
print("############################ The data inserted successfully ", "\n")
cursor.close()
conn.close()
