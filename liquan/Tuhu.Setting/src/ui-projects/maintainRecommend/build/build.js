const fs = require('fs');
fs.renameSync('deploy', 'dist');
console.log('The build folder is ready to be deployed.');
