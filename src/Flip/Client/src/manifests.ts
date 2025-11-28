import { manifests as actionManifests } from "./actions/manifests.js";
import { manifests as langManifests } from "./lang/manifests.js";
import { manifests as modalManifests } from "./modal/manifests.js";
import { manifests as permissionManifests } from "./permissions/manifests.js";

export const manifests: Array<UmbExtensionManifest> = [
  ...actionManifests,
  ...langManifests,
  ...modalManifests,
  ...permissionManifests,
];
