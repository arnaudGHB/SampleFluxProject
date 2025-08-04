#!/bin/sh

# Set the default app name
APP_DLL_NAME=${APP_DLL_NAME:-YourAppName.dll}

# Set additional arguments for your .NET Core application
# You can customize this part based on your app's requirements
# For example, you might want to pass environment variables as app settings
# Here, we're passing environment variables prefixed with 'APPSETTING_'
# as app settings to the .NET Core application

# Extract and filter environment variables prefixed with 'APPSETTING_'
APP_SETTINGS_ARGS=""
for var in $(env | grep '^APPSETTING_' | awk -F= '{print $1}'); do
    value=$(printenv $var)
    APP_SETTINGS_ARGS="$APP_SETTINGS_ARGS --${var#APPSETTING_}=${value}"
done

# Run the application with the specified app name and app settings
exec dotnet "$APP_DLL_NAME" $APP_SETTINGS_ARGS
