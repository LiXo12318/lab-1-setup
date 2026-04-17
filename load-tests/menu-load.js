import http from "k6/http";
import { check } from "k6";

export const options = {
  stages: [
    { duration: "30s", target: 10 },
    { duration: "1m", target: 50 },
    { duration: "1m", target: 50 },
    { duration: "30s", target: 0 },
  ],
  thresholds: {
    checks: ["rate>0.99"],
  },
};

export default function () {
  const res = http.get("http://localhost:5001/api/menu");
  check(res, {
    "status is 200": (r) => r.status === 200,
  });
}
