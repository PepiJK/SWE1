CREATE TABLE temperatures (
    id SERIAL PRIMARY KEY,
    datetime TIMESTAMP,
    value FLOAT
);

/* test data from the last 10 years with random floats ranging from -10 to 25 */
/* 14609 rows */
INSERT INTO temperatures (datetime, value) 
    SELECT day, random()*(-10-25)+25 FROM generate_series(
        '2009-12-18'::timestamp,
        '2019-12-18'::timestamp,
        '6 hour'::interval
    ) day
;









