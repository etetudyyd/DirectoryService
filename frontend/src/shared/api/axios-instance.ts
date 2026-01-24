import axios from "axios";
import { Envelope } from "./envelope";
import { EnvelopeError, ErrorType } from "./errors";

export const apiClient = axios.create({
  baseURL: "http://localhost:8090/api",
  //baseURL: "http://localhost:8080/api",
  headers: { "Content-Type": "application/json" },
});

apiClient.interceptors.response.use(
  (response) => {
    const data = response.data;

    if (
      data?.isError &&
      Array.isArray(data.errorList) &&
      data.errorList.length
    ) {
      throw new EnvelopeError({
        type: data.errorList[0].type as ErrorType,
        messages: data.errorList.map((e: any) => ({
          code: e.code,
          message: e.message,
          invalidField: e.invalidField ?? null,
        })),
      });
    }

    return response;
  },
  (error) => {
    if (error.response?.data) {
      const data = error.response.data;

      if (
        data?.isError &&
        Array.isArray(data.errorList) &&
        data.errorList.length
      ) {
        throw new EnvelopeError({
          type: data.errorList[0].type as ErrorType,
          messages: data.errorList.map((e: any) => ({
            code: e.code,
            message: e.message,
            invalidField: e.invalidField ?? null,
          })),
        });
      }
    }

    return Promise.reject(error);
  },
);
