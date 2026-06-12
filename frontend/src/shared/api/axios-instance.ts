import axios from "axios";
import { EnvelopeError, ErrorType } from "./errors";

type EnvelopeErrorItem = {
  type: ErrorType;
  code: string;
  message: string;
  invalidField?: string | null;
};

export const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:8090/api",
  headers: { "Content-Type": "application/json" },

  paramsSerializer: {
    indexes: null,
  }
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
        messages: (data.errorList as EnvelopeErrorItem[]).map((e) => ({
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
          messages: (data.errorList as EnvelopeErrorItem[]).map((e) => ({
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
