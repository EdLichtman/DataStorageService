FROM microsoft/aspnetcore:2.0.0
ENV BUILD_CONFIG Debug
LABEL maintainer=e.lichtman2@gmail.com \
    Name=data_storage_service-${BUILD_CONFIG} \
    Version=0.0.1
ARG URL_PORT
WORKDIR /app
ENV NUGET_XMLDOC_MODE skip
ENV ASPNETCORE_URLS http://*:${URL_PORT}
COPY ./publish .
ENTRYPOINT [ "dotnet", "ContainerProd.dll" ]