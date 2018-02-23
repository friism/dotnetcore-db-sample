FROM ubuntu:bionic

RUN apt-get update; \
    apt-get install -y --no-install-recommends \
        python \
        openssh \
        curl \
        bash \
    ; \
    rm -rf /var/lib/apt/lists/*

ADD ./.profile.d /app/.profile.d
RUN rm /bin/sh && ln -s /bin/bash /bin/sh

CMD sleep 3600