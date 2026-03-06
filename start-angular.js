// Wrapper: changes CWD to the Angular project directory before starting ng serve
const path = require('path');
process.chdir(path.join(__dirname, 'FuelPriceWizard.UI', 'fuelpricewizard'));
// Replace this script's path with 'ng' so the CLI parses args correctly
process.argv[1] = path.join(process.cwd(), 'node_modules', '@angular', 'cli', 'bin', 'ng.js');
require('./FuelPriceWizard.UI/fuelpricewizard/node_modules/@angular/cli/bin/ng.js');
