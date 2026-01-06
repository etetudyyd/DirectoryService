import axios from "axios";

export const apiCLient = axios.create({
    baseURL: "http://localhost:8090/api",
    headers: { "Content-Type": "application/json" },
});