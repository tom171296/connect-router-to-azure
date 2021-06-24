CSR_CONFIG_DIR=./csr-config-files
CA_CERT_DIR=./ca-cert
PFX_PASSWORD=Passw0rd

### List cert folders and cert names
crt_folders=(
    "./cert/"      #1
)
cert_names=(
    "standalone-router"     #1
)

generate_private_key() {
    FOLDER=$1
    openssl genrsa -out "${FOLDER}"/tls.key 2048
}

generate_csr() {
    FOLDER=$1
    NAME=$2
    # Create a certificate signing request for the client certificate.
    openssl req -new -key "${FOLDER}"/tls.key \
            -out "${FOLDER}"/csr.pem \
            -config "${CSR_CONFIG_DIR}"/"${NAME}".conf
}

sign_csr_with_ca() {
    FOLDER=$1
    NAME=$2    
    # Sign the certificate using the CA.
    openssl x509 -req -in "${FOLDER}"/csr.pem \
            -CA "${CA_CERT_DIR}"/ca.crt -CAkey "${CA_CERT_DIR}"/ca-key.pem \
            -out "${FOLDER}"/tls.crt -CAcreateserial \
            -days 365 \
            -extfile "${CSR_CONFIG_DIR}"/"${NAME}".conf -extensions v3_req
}

### Loop through the lists to generate the private keys, csrs, sign them and then export them to pfx.
for i in "${!crt_folders[@]}"; do
    rm -rf "${crt_folders[i]}"
    mkdir -p "${crt_folders[i]}"

    echo started generating private key file into folder "${crt_folders[i]}" for cert "${cert_names[i]}"
    generate_private_key "${crt_folders[i]}" &&
        echo successfully generated private key file into folder "${crt_folders[i]}" for cert "${cert_names[i]}"

    echo started creating certificate signing request into folder "${crt_folders[i]}" for cert "${cert_names[i]}"
    generate_csr "${crt_folders[i]}" "${cert_names[i]}" &&
        echo successfully generated certificate signing request into folder "${crt_folders[i]}" for cert "${cert_names[i]}"

    echo started signing csr file into folder "${crt_folders[i]}" for cert "${cert_names[i]}"
    sign_csr_with_ca "${crt_folders[i]}" "${cert_names[i]}" &&
        echo successfully signed csr file into folder "${crt_folders[i]}" for cert "${cert_names[i]}"
done
