let path = require('path');
let fs = require('fs');
let copy = require('./copy');
let folders = require('./folders');
// var jsonConcat = require('json-concat');
// const merge = require('deepmerge')


// Exit vars
let done = false;

// Paths
let root = folders.getInitCwd();
let src = path.join(__dirname, '..', 'src', 'PackageUnityProject', 'Assets');
let pkg = path.join(root, 'Assets', 'TempPackage');
let isModule = fs.existsSync(path.join(root, 'package.json'));
let srcManifest = path.join(__dirname, '..', 'src', 'PackageUnityProject', 'Packages');
let destManifest = path.join(root, 'Packages_npm');

// Create folder if missing
if (isModule) {
  copy.copy(src, pkg).then(() => {
    done = true;
  }, (err) => {
    console.error(err);
    process.exit(1);
  });

   copy.copy(srcManifest, destManifest).then(() => {
    done = true;
  }, (err) => {
    console.error(err);
    process.exit(1);
  });

  // const obj = JSON.parse(fs.);
  // const merged = (srcManifest, destManifest);
  // console.log('Merged json : ' + merged);
//   jsonConcat({
//   	src: srcManifest,
//   	dest: destManifest
//   }, function (json) {
//     console.log(json);
// });
} else {
  console.log('Skip install, not a project: ' + root);
  done = true;
}

(function wait() {
  if (!done) setTimeout(wait, 100);
})();
