const backofficePath = './src/Flip/Backoffice';

// two directories up to the test sites
// but build into /src

export const paths = {
    js: [`${backofficePath}/**/*.ts`],
    viewsDev: [`${backofficePath}/**/*.html`],
    viewsProd: [`${backofficePath}/**/*.html`, `!${backofficePath}/**/components/**/*.html`],
    scss: `${backofficePath}/**/*.scss`,
    lang: `./src/Flip/Lang/*.xml`,
    manifest: './src/Flip/package.manifest',
    dest: './App_Plugins/Flip/',
    site: '../../Flip.Umbraco8/App_Plugins/Flip/',
    siteNetCore: '../../Flip.Umbraco9/App_Plugins/Flip/',
};

export const config = {
    hash: new Date().toISOString().split('').reduce((a, b) => (((a << 5) - a) + b.charCodeAt(0)) | 0, 0).toString().substring(1)
};
