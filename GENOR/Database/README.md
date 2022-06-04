# GENOR database

## Install

- Install PostgreSQL (https://www.postgresql.org/)
- Create a database, and add the following tables:

```
CREATE TABLE category (
	id serial PRIMARY KEY,
	name VARCHAR (50) NOT NULL
);

CREATE TABLE indicator (
	id serial PRIMARY KEY,
	name VARCHAR (50) NOT NULL
);

CREATE TABLE category_indicator (
	category_id INT NOT NULL,
	indicator_id INT NOT NULL,
	FOREIGN KEY (category_id) REFERENCES category (id),
	FOREIGN KEY (indicator_id) REFERENCES indicator (id)
);
```

- You will have to update the `Server/database/main.py` file with the address of your database.