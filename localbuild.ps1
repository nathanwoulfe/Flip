## clean up from previous runs
rm -r -force nupkgs
mkdir nupkgs

## install backoffice dependencies
cd ./src/Flip/Backoffice
npm run prod
cd ../../../

## pack the container 
dotnet pack ./src/Flip/Flip.csproj -c Release -o nupkgs