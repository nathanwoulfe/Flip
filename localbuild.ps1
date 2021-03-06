## clean up from previous runs
rm -r -force nupkgs
rm -r -force ./src/Flip.Backoffice/App_Plugins
mkdir nupkgs

## install backoffice dependencies
cd ./src/Flip.Backoffice
## npm install
npm run prod
cd ../../

## pack individually to avoid flip.site blowing up
dotnet pack ./src/Flip.Web/Flip.Web.csproj -c Release -o nupkgs
dotnet pack ./src/Flip.Backoffice/Flip.Backoffice.csproj -c Release -o nupkgs

## pack the container 
dotnet pack ./src/Flip/Flip.csproj -c Release -o nupkgs