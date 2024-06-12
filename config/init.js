rs.status();
db.createUser({user: 'admin', pwd: 'admin', roles: [ { role: 'root', db: 'admin' } ], mechanisms: [ "SCRAM-SHA-256" ]});

db = db.getSiblingDB('rent');

db.createUser({
  user: 'admin',
  pwd: 'admin',
  roles: [{ role: 'readWrite', db: 'rent' }],
  mechanisms: [ "SCRAM-SHA-256" ]
});

db.createCollection('vehicles');

db.getCollection('vehicles').remove({});
