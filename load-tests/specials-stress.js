import http from "k6/http";
import { check } from "k6";

export const options = {
  stages: [
    { duration: "10s", target: 5 },
    { duration: "20s", target: 150 },
    { duration: "15s", target: 10 },
    { duration: "20s", target: 200 },
    { duration: "15s", target: 0 },
  ],
  thresholds: {
    checks: ["rate>0.99"],
  },
};

export default function () {
  const res = http.get("http://localhost:5001/api/specials/today");
  check(res, {
    "status is 200": (r) => r.status === 200,
  });
}
