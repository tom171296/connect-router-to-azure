FROM fedora:33

# Install QPID
RUN yum install -y qpid-dispatch-router-1.16.0 qpid-dispatch-console cyrus-sasl-plain-2.1.27-6.fc33.x86_64 && yum clean all

# Start the dispatch router
ENTRYPOINT ["qdrouterd", "-c", "etc/qpid-dispatch/qdrouterd.conf"]