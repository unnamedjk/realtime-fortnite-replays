docker run -i --init `
    --name fndb `
    -e LICENSE_KEY='BDIwNTY2MzczYmQ3ZTQ0OTBhNzgzYjEzYzI2ZjFlMWYwAAAAAAAAAAAAAAAAAAAAAAMwNQIYRaxfeh15wzJd+mHRW78Xv6PGnh1vlSnSAhkAk/Fw+QJCKDeEhFb5ed7DvMhGvijPZrHFAA==' `
    -e ROOT_PASSWORD=fndemo `
    -e START_AFTER_INIT=Y `
    -p 3306:3306 -p 8080:8080 `
    singlestore/cluster-in-a-box
