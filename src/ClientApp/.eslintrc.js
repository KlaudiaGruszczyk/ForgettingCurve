module.exports = {
  root: true,
  ignorePatterns: ["node_modules/**", "dist/**", ".angular/**", "coverage/**"],
  extends: [
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "prettier"
  ],
  parser: "@typescript-eslint/parser",
  parserOptions: {
    ecmaVersion: 2020,
    sourceType: "module",
    project: ["tsconfig.json"]
  },
  plugins: ["@typescript-eslint"],
  rules: {
    "@typescript-eslint/explicit-function-return-type": "off",
    "@typescript-eslint/explicit-module-boundary-types": "off",
    "@typescript-eslint/no-explicit-any": "warn",
    "@typescript-eslint/no-unused-vars": "warn"
  }
}; 