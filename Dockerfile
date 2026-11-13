FROM squidfunk/mkdocs-material:9.7.7

# Plugins are listed in requirements.txt (shared with .sandbox/setup.sh for Docker-less setups).
# Build deps are only needed to compile lxml (required for mkdocs-git-committers-plugin-2).
COPY requirements.txt /tmp/requirements.txt
RUN apk add --no-cache --virtual .build-deps gcc libc-dev libxslt-dev && \
    apk add --no-cache libxslt && \
    pip install --no-cache-dir -r /tmp/requirements.txt && \
    apk del .build-deps && \
    rm /tmp/requirements.txt

RUN git config --global --add safe.directory /github/workspace

EXPOSE 8000

# override inherited entrypoint (which is mkdocs), we want to have full control over
# what is executed using docker run's command parameters
ENTRYPOINT [] 

# Run the "mkdocs serve" command at the specified port.
# (to build the docs without serve: just override the cmd with "mkdocs build" when
# executing docker run. This can be used when runnig this docker image as github action
# in the CI pipeline)
CMD ["mkdocs", "serve", "--dev-addr=0.0.0.0:8000"]