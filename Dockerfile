# 🏗️ Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# نسخ جميع الملفات داخل `LeaveRequestSystem/`
COPY . .

# الانتقال إلى مجلد `LeaveRequestSystem` داخل الحاوية
WORKDIR /app

# استعادة التبعيات
RUN dotnet restore

# تنفيذ البناء والنشر
RUN dotnet publish -c Release -o /publish --no-restore

# 🚀 Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# 🛠️ ضبط المنفذ ليعمل مع Railway
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

# نسخ الملفات المنشورة من مرحلة البناء
COPY --from=build /publish .

# تشغيل التطبيق
ENTRYPOINT ["dotnet", "LeaveRequestSystem.dll"]