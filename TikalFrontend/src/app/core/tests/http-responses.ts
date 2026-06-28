export type HttpResponseData = {
  status: number;
  statusText: string;
};

export const BAD_REQUEST = {
  status: 400,
  statusText: 'BadRequest',
};

export const UNAUTHORIZED = {
  status: 401,
  statusText: 'Unauthorized',
};

export const PAYMENT_REQUIRED = {
  status: 402,
  statusText: 'PaymentRequired',
};

export const FORBIDDEN = {
  status: 403,
  statusText: 'Forbidden',
};

export const NOT_FOUND = {
  status: 404,
  statusText: 'NotFound',
};

export const METHOD_NOT_ALLOWED = {
  status: 405,
  statusText: 'MethodNotAllowed',
};

export const NOT_ACCEPTABLE = {
  status: 406,
  statusText: 'NotAcceptable',
};

export const ERROR_RESPONSES = [
  BAD_REQUEST,
  UNAUTHORIZED,
  PAYMENT_REQUIRED,
  FORBIDDEN,
  NOT_FOUND,
  METHOD_NOT_ALLOWED,
  NOT_ACCEPTABLE,
];
