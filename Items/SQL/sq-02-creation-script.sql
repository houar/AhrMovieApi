
---
-- Insert movies into the Movies table
INSERT INTO Movies (Id, Title, Slug, YearOfRelease) VALUES
  ('ad379b9b-257e-445c-a9ea-10da301f2386', 'Toy Story', 'toy-story-1995', 1995),
  ('578be646-61a6-4bd3-b824-e4556b51b294', 'Jumanji', 'jumanji-1995', 1995),
  ('af3b265c-283a-4c91-a3c8-9b6b942509b9', 'Grumpier Old Men', 'grumpier-old-men-1995', 1995),
  ('514fd75d-76a4-4484-a811-8d71f3baa400', 'Waiting to Exhale', 'waiting-to-exhale-1995', 1995),
  ('ac71ae5a-144a-4df1-877c-5e52851f07dc', 'Father of the Bride Part II', 'father-of-the-bride-part-ii-1995', 1995),
  ('92c3238c-14a2-4de1-aaea-57e5d668cb05', 'Heat', 'heat-1995', 1995),
  ('12e8615d-e4b3-4be4-b949-7d29407fc317', 'Sabrina', 'sabrina-1995', 1995),
  ('03610763-3f93-4e7f-a7c0-7759e9ab25b0', 'Tom and Huck', 'tom-and-huck-1995', 1995),
  ('b173a016-1599-49cf-a34e-cf32716ec76f', 'Sudden Death', 'sudden-death-1995', 1995),
  ('1bb2dc70-bd13-4586-bc1c-cb04582002e9', 'GoldenEye', 'goldeneye-1995', 1995);
---

---
-- Insert genres into the Genres table
INSERT INTO Genres (movieId, name) VALUES
  ('ad379b9b-257e-445c-a9ea-10da301f2386', 'Adventure'),
  ('ad379b9b-257e-445c-a9ea-10da301f2386', 'Animation'),
  ('ad379b9b-257e-445c-a9ea-10da301f2386', 'Children'),
  ('ad379b9b-257e-445c-a9ea-10da301f2386', 'Comedy'),
  ('ad379b9b-257e-445c-a9ea-10da301f2386', 'Fantasy'),

  ('578be646-61a6-4bd3-b824-e4556b51b294', 'Adventure'),
  ('578be646-61a6-4bd3-b824-e4556b51b294', 'Children'),
  ('578be646-61a6-4bd3-b824-e4556b51b294', 'Fantasy'),

  ('af3b265c-283a-4c91-a3c8-9b6b942509b9', 'Comedy'),
  ('af3b265c-283a-4c91-a3c8-9b6b942509b9', 'Romance'),

  ('514fd75d-76a4-4484-a811-8d71f3baa400', 'Comedy'),
  ('514fd75d-76a4-4484-a811-8d71f3baa400', 'Drama'),
  ('514fd75d-76a4-4484-a811-8d71f3baa400', 'Romance'),

  ('ac71ae5a-144a-4df1-877c-5e52851f07dc', 'Comedy'),

  ('92c3238c-14a2-4de1-aaea-57e5d668cb05', 'Action'),
  ('92c3238c-14a2-4de1-aaea-57e5d668cb05', 'Crime'),
  ('92c3238c-14a2-4de1-aaea-57e5d668cb05', 'Thriller'),

  ('12e8615d-e4b3-4be4-b949-7d29407fc317', 'Comedy'),
  ('12e8615d-e4b3-4be4-b949-7d29407fc317', 'Romance'),

  ('03610763-3f93-4e7f-a7c0-7759e9ab25b0', 'Adventure'),
  ('03610763-3f93-4e7f-a7c0-7759e9ab25b0', 'Children'),

  ('b173a016-1599-49cf-a34e-cf32716ec76f', 'Action'),

  ('1bb2dc70-bd13-4586-bc1c-cb04582002e9', 'Action'),
  ('1bb2dc70-bd13-4586-bc1c-cb04582002e9', 'Adventure'),
  ('1bb2dc70-bd13-4586-bc1c-cb04582002e9', 'Thriller');
---
