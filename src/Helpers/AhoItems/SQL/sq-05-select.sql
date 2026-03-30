
SELECT * FROM movies mo
WHERE mo.title IS NULL OR
mo.yearofrelease IS NULL;

SELECT * FROM movies mo
LEFT JOIN genres ge ON mo.id = ge.movieid
WHERE ge.movieid IS NULL;

SELECT * FROM movies mo
WHERE mo.slug LIKE 'nick-the-greek%';
