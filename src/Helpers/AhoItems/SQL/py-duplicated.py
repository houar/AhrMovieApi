import json
from collections import defaultdict
from pathlib import Path

# Get the directory where the .py file is located
script_dir = Path(__file__).parent

# Load the JSON file
with open(f'{script_dir}\\jsn-newmovies.json', 'r') as file:
    data = json.load(file)

print("############################ Find duplicate entries in the file: ")
print(">>> ", file.name, "\n")

# Dictionaries to track duplicates
slug_count = defaultdict(list)  # Dictionary to store slugs and their corresponding entries
id_count = defaultdict(list)    # Dictionary to store ids and their corresponding entries

# Step 2: Populate the dictionaries
for movie in data:
    slug_count[movie['Slug']].append(movie)
    id_count[movie['Id']].append(movie)

# Step 3: Find and print duplicate slugs
print("Duplicate Slugs:")
print("----------------\n")
for slug, movies in slug_count.items():
    if len(movies) > 1:
        print(f"Slug: {slug}\n")
        for movie in movies:
            print(f"  - Id : {movie['Id']}")

# Step 4: Find and print duplicate ids
print("\nDuplicate IDs:")
print("--------------\n")
for movie_id, movies in id_count.items():
    if len(movies) > 1:
        print(f"ID: {movie_id}")
        for movie in movies:
            print(f"  - Slug : {movie['Slug']}")
# end
