SELECT hash_val AS hash_1 FROM napkins.images WHERE BIT_COUNT(hash_1 ^ hash_2) < 3;
