# Note, this seems to throw a QEMU error when running on arm Mac
# We may need to upgrade or only build this on x64
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as builder
COPY . /
RUN cd "ROID-Forum-Server"
RUN dotnet build --configuration Release

FROM mcr.microsoft.com/dotnet/core/sdk:2.2
COPY --from=builder / /
WORKDIR "/ROID-Forum-Server"
CMD dotnet run --configuration Release