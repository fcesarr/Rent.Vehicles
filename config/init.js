// rs.initiate(
//   {
//     _id: "rs0",
//     members: [
//       { _id: 0, host: "database-mongo-1:27017" },
//       { _id: 1, host: "database-mongo-2:27017" },
//       { _id: 2, host: "database-mongo-3:27017" }
//     ]
//   }
// )

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


db = db.getSiblingDB('rent-tests');

db.createUser({
  user: 'admin',
  pwd: 'admin',
  roles: [{ role: 'readWrite', db: 'rent-tests' }],
  mechanisms: [ "SCRAM-SHA-256" ]
});

db.createCollection('vehicles');

db.getCollection('vehicles').remove({});