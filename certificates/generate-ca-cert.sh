PFX_PASSWORD=Passw0rd

# Create a new directory for ca certs to be used by both zones
rm -rf ./ca-cert/
mkdir -p ./ca-cert/

# Create a private key for the CA.
openssl genrsa -out ./ca-cert/ca-key.pem 4096

# Create a certificate signing request for the CA.
openssl req -new -sha256 -batch -key ./ca-cert/ca-key.pem -out ./ca-cert/ca-csr.pem -subj "/C=NL/ST=Utrecht/L=Utrecht/O=InfoSupport/CN=InfoSupprt Test Root"

# Self sign the CA certificate.
openssl x509 -req -in ./ca-cert/ca-csr.pem \
        -signkey ./ca-cert/ca-key.pem -out ./ca-cert/ca.crt \
        -days 365
