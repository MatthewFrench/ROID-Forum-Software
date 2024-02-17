# Note, this seems to throw a QEMU error when running on arm Mac
# We may need to upgrade or only build this on x64
FROM mcr.microsoft.com/dotnet/sdk:8.0 as builder
COPY . /
RUN cd "ROID-Forum-Server"
RUN dotnet build --configuration Release

FROM mcr.microsoft.com/dotnet/runtime:8.0
COPY --from=builder / /
WORKDIR "/ROID-Forum-Server"
CMD dotnet run --configuration Release